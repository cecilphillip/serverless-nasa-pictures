import datetime
import logging
import os
from datetime import datetime

import azure.functions as func
import emoji
import pytz
import requests
from requests.exceptions import Timeout
from twilio.rest import Client

account_sid = os.getenv("TWILIO_ACCOUNT_SID")
auth_token = os.getenv("TWILIO_AUTH_TOKEN")
nasa_api_key = os.getenv("NASA_API_KEY")
my_twilio_number = os.getenv("MY_TWILIO_NUMBER")
receiver_number = os.getenv("RECEIVER_NUMBER")


def main(timerreq: func.TimerRequest) -> None:
    my_timezone = pytz.timezone("America/New_York")

    current = datetime.now(tz=my_timezone).strftime('%Y-%m-%d')
    target = '2020-03-30'

    current_converted = datetime.strptime(current, '%Y-%m-%d')
    target_converted = datetime.strptime(target, '%Y-%m-%d')

    diff = current_converted - target_converted

    # Get data from Nasa
    try:
        response = requests.get(
            'https://api.nasa.gov/planetary/apod?api_key=' + nasa_api_key, timeout=1.0)
        response.raise_for_status()
        response_json = response.json()
    except (Timeout, requests.HTTPError) as ex:
        # replace these with better logging
        print('the request to the service failed')
        print(ex)
        return

    # variables for the text message
    picture = response_json['url']
    title = response_json['title']
    rocket = emoji.emojize(":rocket:")

    # Twilio account credentials
    client = Client(account_sid, auth_token)

    # function if youtube is in the URL
    def youtube():
        message = client.messages \
            .create(
                body=f'Check out this video of: {title}! \n\n {picture} \n\n Btw...T{diff.days} days until we go to NASA! {rocket}',
                from_=my_twilio_number,

                to=receiver_number
            )
        print(message.sid)

    # function if youtube is NOT in the URL
    def image():
        message = client.messages \
            .create(
                body=f'Today\'s picture is of: {title}! \n\n Btw...T{diff.days} days until we go to NASA! {rocket}',
                from_=my_twilio_number,
                media_url=[f'{picture}'],
                to=receiver_number,
            )
        print(message.sid)

    if "youtube" in picture:
        youtube()
    else:
        image()
