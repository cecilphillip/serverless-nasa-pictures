# Picture of The Day Sample usinig Python, Twilio and Azure Functions

> This code is inspired by a blog post written by @aprilspeight on the Twilio blog [Create a NASA Astronomy Picture of the Day Scheduled SMS with Python, Twilio and Azure Functions](https://www.twilio.com/blog/nasa-astronomy-picture-day-scheduled-sms-python-twilio-azure-functions)

This repo contains a sample application, written is various programming languages, that shows how to use an Azure Functions TimerTrigger to call the NASA  Astronomy Picture of the Day (APOD) API. Afterwards, Twilio is used to send a SMS message with the retrived picture.

## What do you need?
- [Visual Studio Code](https://code.visualstudio.com/)
- The [Python extension for Visual Studio Code](https://marketplace.visualstudio.com/items?itemName=ms-python.python)
- [Python 3.7 or above](https://www.python.org/downloads/)
- An API key for the [NASA APIS](https://api.nasa.gov/)
- The [Azure Functions Core Tools](https://docs.microsoft.com/en-us/azure/azure-functions/functions-run-local?tabs=linux%2Ccsharp%2Cbash#install-the-azure-functions-core-tools)
