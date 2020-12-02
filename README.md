# Observability Lab

This repository is used to experiment with and demo observability tools for dotnet.

## Getting Started

1. Run `docker-compose up`
2. Run `sh start-services-with-load-simulator.sh`
3. Open `http://localhost:3000` to access Grafana. Login with user `admin` and password `admin`
4. Add the following sources to Grafana
    - Prometheus - `http://localhost:9091`
    - Jaeger - `http://localhost:16686`
    - Loki - `http://localhost:3100`

You should now see metrics, traces and logs for the currency microservices. You can also navigate to `http://localhost:5001/swagger` to try the currency conversion API manually.

## Troubleshooting

### HTTP Exceptions When Running `start-services-with-load-simulator.sh`

This probably means that you don't have the developer certificate installed.

### To Fix

1. Open `CurrencyApi/CurrencyApi.csproj` with Visual Studio.
2. Select `CurrencyApi` in the Debug Target Selector (play button in the top bar)
3. Start the debugger (hit the play button)
4. When prompted, accept the certificate