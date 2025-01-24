FROM mcr.microsoft.com/dotnet/sdk:8.0

ARG LBHPACKAGESTOKEN
ENV LBHPACKAGESTOKEN=$LBHPACKAGESTOKEN
WORKDIR /app

# Copy csproj and restore as distinct layers
COPY ./ActivityListener/ActivityListener.csproj ./
COPY /NuGet.Config /root/.nuget/NuGet/NuGet.Config
RUN dotnet restore ./ActivityListener.csproj


# Copy everything else and build
COPY ./ActivityListener ./
RUN dotnet build -c Release -o out ./ActivityListener.csproj
RUN dotnet publish -c Release -o out ./ActivityListener.csproj

EXPOSE ${PORT:-3000}
CMD ASPNETCORE_URLS=http://+:${PORT:-3000} dotnet ./out/ActivityListener.dll
