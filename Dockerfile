FROM dotnet/sdk:10.0-debian-13 AS restore
ARG BUILD_CONFIGURATION=Debug
WORKDIR /repo

COPY ["src/Feast.CodeAnalysis.Console/Feast.CodeAnalysis.Console.csproj", "src/Feast.CodeAnalysis.Console/"]
COPY ["src/Feast.CodeAnalysis.LiteralGenerator/Feast.CodeAnalysis.LiteralGenerator.csproj", "src/Feast.CodeAnalysis.LiteralGenerator/"]
COPY ["src/Feast.CodeAnalysis.TestGenerator/Feast.CodeAnalysis.TestGenerator.csproj", "src/Feast.CodeAnalysis.TestGenerator/"]
COPY ["src/Feast.CodeAnalysis.SourceGenerators/Feast.CodeAnalysis.SourceGenerators.csproj", "src/Feast.CodeAnalysis.SourceGenerators/"]
COPY ["src/Feast.CodeAnalysis.ScriptingGenerator/Feast.CodeAnalysis.ScriptingGenerator.csproj", "src/Feast.CodeAnalysis.ScriptingGenerator/"]
COPY ["src/Feast.CodeAnalysis.Scripting/Feast.CodeAnalysis.Scripting.csproj", "src/Feast.CodeAnalysis.Scripting/"]
RUN dotnet restore "src/Feast.CodeAnalysis.Console/Feast.CodeAnalysis.Console.csproj"

FROM restore AS build
WORKDIR /repo
COPY . .
RUN dotnet build "src/Feast.CodeAnalysis.Console/Feast.CodeAnalysis.Console.csproj" -c $BUILD_CONFIGURATION -o /app/build
CMD ["/app/build/Feast.CodeAnalysis.Console"]