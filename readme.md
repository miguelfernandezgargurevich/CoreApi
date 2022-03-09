# Microservice Netcore

Include a README file explaining the general aspects of your code design and the instruction on how to run
your application

## General aspects

- ApiRest architecture Based on .NetCore 3.1**.
- Includes Bearer Token validation. (Dummy)
- Inputs/Outputs have JSON structure.

## Instruction
- Download the code from GIT Repository: https://github.com/miguelgargurevich/CoreApi.git
- Publish files: "bin\Release\netcoreapp3.1\publish\" or Run the aplication using Visual Studio 2019

- URL test using IIS Express
  SSL: https://localhost:44350/
  No SSL: http://localhost:44347/

- Endpoint: http://localhost:44347/core-api/procesar
- Use Postman to test sending JSON body:
{
"foundingType": "SME",
"cashBalance": 435.30,
"monthlyRevenue": 4235.45,
"requestedCreditLine": 500,
"requestedDate": "2022-03-07T20:46:14.900Z"
}

- The Response will be like this:
{
    "code": "1",
    "message": "Acepted: Credit line authorized: 847.09",
    "status": 200
}

- IMPORTANT: For Authorization, must includes Dummy Bearer Token in request
example Dummy Bearer Token: 123456

## Requirements

- No database instance is required

## Enviroment variables

- No Enviroment variables are required:


