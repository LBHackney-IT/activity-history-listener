# Activity History Listener

.NET 10 AWS Lambda that consumes housing-solution events from SQS (published via SNS) and writes an activity log to the DynamoDB `ActivityHistory` table. The log supports audit lookup and debugging of failed or missing events.

## Tech stack

- .NET 10 (`net10.0`, AWS Lambda `dotnet10`)
- Amazon SQS / SNS
- DynamoDB, with DynamoDB Local for local development
- AWS X-Ray

## Layout

One listener project, organised in layers:

- `ActivityListener/Boundary` — event contracts
- `ActivityListener/UseCase` — message processing
- `ActivityListener/Gateway` — DynamoDB access
- `ActivityListener/SqsFunction.cs` — Lambda entry point
- `ActivityListener.Tests` — unit and end-to-end tests (DynamoDB Local)

## Prerequisites

- .NET 10 SDK
- Docker
- `LBHPACKAGESTOKEN` for the private Hackney NuGet feed (`NuGet.Config`)

## Run and test

Start DynamoDB Local, then run the tests:

```bash
docker compose up dynamodb-database -d
dotnet test ActivityListener.sln
```

Run the listener and DynamoDB Local together:

```bash
docker compose up --build
```

Package the Lambda (from `ActivityListener/`):

```bash
./build.sh
```
