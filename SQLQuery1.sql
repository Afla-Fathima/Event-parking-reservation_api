create database EventParkingReservationDb_Fresh2026;
GO

SELECT SUSER_SNAME(owner_sid) AS DatabaseOwner
FROM sys.databases
WHERE name='EventParkingReservationDb_Fresh2026';