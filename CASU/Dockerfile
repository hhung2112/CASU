# --- Giai đoạn Build ứng dụng ---
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy file csproj và restore các NuGet packages
COPY ["CASU.csproj", "./"]
RUN dotnet restore "CASU.csproj"

# Copy toàn bộ mã nguồn còn lại
COPY . .
RUN dotnet build "CASU.csproj" -c Release -o /app/build

# --- Giai đoạn Publish ---
FROM build AS publish
RUN dotnet publish "CASU.csproj" -c Release -o /app/publish /p:UseAppHost=false

# --- Giai đoạn Chạy ứng dụng trên Runtime ---
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=publish /app/publish .

# Cấu hình cổng chạy cho Render
ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080

ENTRYPOINT ["dotnet", "CASU.dll"]