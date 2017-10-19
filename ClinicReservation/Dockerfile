FROM bitnp/aspnetcore:2.0
ARG source
WORKDIR /app
EXPOSE 80
COPY ${source:-obj/Docker/publish} .
COPY ${source:-ClinicReservation/Views} .
ENTRYPOINT ["dotnet", "ClinicReservation.dll"]
