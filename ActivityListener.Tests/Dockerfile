FROM mcr.microsoft.com/dotnet/sdk:6.0

# disable microsoft telematry
ENV DOTNET_CLI_TELEMETRY_OPTOUT='true'

ENV DynamoDb_LocalMode='true'
ARG LBHPACKAGESTOKEN
ENV LBHPACKAGESTOKEN=$LBHPACKAGESTOKEN

ARG SONAR_TOKEN
ENV SONAR_TOKEN=$SONAR_TOKEN

WORKDIR /app

RUN apt-get update && apt-get install -y openjdk-17-jdk
RUN dotnet tool install --global dotnet-sonarscanner --version 5.6.0
ENV PATH="$PATH:/root/.dotnet/tools"

RUN dotnet sonarscanner begin /k:"LBHackney-IT_activity-history-listener" /o:"lbhackney-it" /d:sonar.host.url=https://sonarcloud.io /d:sonar.login="${SONAR_TOKEN}"


# Copy csproj and restore as distinct layers
COPY ./ActivityListener.sln ./
COPY ./ActivityListener/ActivityListener.csproj ./ActivityListener/
COPY ./ActivityListener.Tests/ActivityListener.Tests.csproj ./ActivityListener.Tests/
COPY /NuGet.Config /root/.nuget/NuGet/NuGet.Config

RUN dotnet restore ./ActivityListener/ActivityListener.csproj
RUN dotnet restore ./ActivityListener.Tests/ActivityListener.Tests.csproj

# Copy everything else and build
COPY . .

RUN dotnet build -c Release -o out ActivityListener/ActivityListener.csproj
RUN dotnet build -c debug -o out ActivityListener.Tests/ActivityListener.Tests.csproj

CMD dotnet test
RUN dotnet sonarscanner end /d:sonar.login="${SONAR_TOKEN}"
