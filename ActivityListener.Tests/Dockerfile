FROM mcr.microsoft.com/dotnet/sdk:6.0

ENV DOTNET_CLI_TELEMETRY_OPTOUT='true'
ENV DynamoDb_LocalMode='true'

ARG LBHPACKAGESTOKEN
ENV LBHPACKAGESTOKEN=$LBHPACKAGESTOKEN

WORKDIR /app

ENV PATH="$PATH:/root/.dotnet/tools"

# Copy csproj and restore as distinct layers
COPY ./ActivityListener.sln ./
COPY ./ActivityListener/ActivityListener.csproj ./ActivityListener/
COPY ./ActivityListener.Tests/ActivityListener.Tests.csproj ./ActivityListener.Tests/
COPY /NuGet.Config /root/.nuget/NuGet/NuGet.Config

RUN dotnet restore

COPY . .

RUN dotnet build -c Release -o out ActivityListener/ActivityListener.csproj
RUN dotnet build -c debug -o out ActivityListener.Tests/ActivityListener.Tests.csproj

ENTRYPOINT ["dotnet", "test", "--collect", "XPlat Code Coverage;Format=opencover", "--results-directory", "./coverage"]
