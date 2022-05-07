from os import getenv
import psycopg2
from contextlib import closing
from json import load
import requests
from dotenv import load_dotenv
from psycopg2 import sql


MAIN_API_URL = 'https://www.alphavantage.co/query?function=OVERVIEW&symbol='
BALANCE_SHEET_API_URL = 'https://www.alphavantage.co/query?function=BALANCE_SHEET&symbol='


class CompanyDBEntity(object):

    def __init__(self, symbol: str):
        self.symbol = symbol

    def get_info_on_company(self, api_key: str, api_url: str):
        url = f'{api_url}{self.symbol}&apikey={api_key}'

        r = requests.get(url)
        r.raise_for_status()

        info = r.json()

        return info


class Company(CompanyDBEntity):

    def __init__(self, symbol: str, api_key: str):
        super().__init__(symbol)

        company_info = self.get_info_on_company(api_key, MAIN_API_URL)

        self.name = company_info['Name']
        self.address = company_info['Address']
        self.sector = company_info['Sector']
        self.industry = company_info['Industry']
        self.country = company_info['Country']
        self.description = company_info['Description']

    def upload_to_db(self, cursor):
        insert = sql.SQL('INSERT INTO companies (symbol, name, address, sector, industry, country, description) VALUES {}'
                         .format(sql.SQL(',')
                                 .join([self.symbol, self.name, self.address, self.sector, self.industry, self.country, self.description])))

        cursor.execute(insert)

        return True


class BalanceSheet(CompanyDBEntity):

    def __init__(self, symbol: str, api_key: str):
        super().__init__(symbol)
        try:
            company_info = self.get_info_on_company(api_key=api_key, api_url=BALANCE_SHEET_API_URL)['annualReports'][0]
            self.reported_currency = company_info['reportedCurrency']
            self.total_current_assets = company_info['totalCurrentAssets']
            self.total_assets = company_info['totalAssets'],
            self.total_current_liabilities = company_info['totalCurrentLiabilities'],
            self.total_liabilities = company_info['totalLiabilities'],
            self.common_stock = company_info['commonStock']
        except requests.exceptions.BaseHTTPError:
            print('Error')

    def upload_to_db(self, cursor):
        text_query = "INSERT INTO balance_sheets (symbol, reported_currency, total_current_assets, total_assets, total_current_liabilities, total_liabilities, common_stock) VALUES (%s, %s, %s, %s, %s, %s, %s)"
        query = cursor.mogrify(text_query, [self.symbol, self.reported_currency, self.total_current_assets, self.total_assets, self.total_current_liabilities, self.total_liabilities, self.common_stock])

        cursor.execute(query)

        return True


if __name__ == '__main__':
    load_dotenv()
    API_KEY = getenv('API_KEY')

    with closing(psycopg2.connect(
        host="ec2-34-242-8-97.eu-west-1.compute.amazonaws.com",
        port=5432,
        database="dehncl3mtaaib5",
        user="pkpjijzdqxxplx",
        password="password"
    )) as conn:
        conn.autocommit = True

        curr = conn.cursor()

        with open('symbols.json', 'r') as json_file:
            symbols = load(json_file)
            companies = []
            balance_sheets = []

            for company_symbol in symbols[:5]:
                balance_sheets.append(BalanceSheet(company_symbol, API_KEY))

            for balance_sheet in balance_sheets:
                balance_sheet.upload_to_db(curr)

