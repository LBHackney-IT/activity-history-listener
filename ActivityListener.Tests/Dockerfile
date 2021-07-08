FROM mcr.microsoft.com/dotnet/core/sdk:3.1

# disable microsoft telematry
ENV DOTNET_CLI_TELEMETRY_OPTOUT='true'

ENV DynamoDb_LocalMode='true'
ARG LBHPACKAGESTOKEN
ENV LBHPACKAGESTOKEN=$LBHPACKAGESTOKEN
WORKDIR /app

# Copy csproj and restore as distinct layers
COPY ./ActivityListener.sln ./
COPY ./ActivityListener/ActivityListener.csproj ./ActivityListener/
COPY ./ActivityListener.Tests/ActivityListener.Tests.csproj ./ActivityListener.Tests/
COPY /NuGet.Config /root/.nuget/NuGet/NuGet.Config

RUN dotnet restore ./ActivityListener/ActivityListener.csproj
RUN dotnet restore ./ActivityListener.Tests/ActivityListener.Tests.csproj

# Copy everything else and build
COPY . .

RUN dotnet build -c debug -o out ActivityListener.Tests/ActivityListener.Tests.csproj

CMD dotnet test
