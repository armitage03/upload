# File Upload Web API Project

A service to upload transaction data from csv and xml files into database and query transactions by specified criteria.

## Getting Started

In order to run and test the project, you need to change database configuration. After that need to create necessary database table by run the script provided in this project.

### Change database configuration

You need to open Web.config file under FileUpload folder. Change DBServerName and DatabaseName.

```
<add name="DefaultConnection" connectionString="data source=DBServerName;initial catalog=DatabaseName;Integrated Security=True" providerName="System.Data.SqlClient" />
    
```

### Run table script into your database

Open Table Script.sql file under FileUpload\DbScript folder. Run this script into your test database.

```
CREATE TABLE [dbo].[Transaction](
	[Id] [int] PRIMARY KEY IDENTITY(1,1) NOT NULL,
	[Amount] [decimal](18, 2) NOT NULL,
	[CurrencyCode] [nvarchar](max) NOT NULL,
	[TransactionDate] [datetime] NOT NULL,
	[IsCSV] [bit] NOT NULL,
	[Status] [int] NOT NULL,
	[Code] [nvarchar](max) NOT NULL
 );
```
## Built With

* [ASP.NET Web API] - The web api framework used
* [MVC] - The web framework used
* [Entity Framework] - For database CRUD
* [Bootstrap 3.3.7] - For responsive web design
* [JQuery 3.3.1] - For datepicker and javascript
* [LumenWorksCsvReader 4.0.0] - For csv file reader

## Versioning

We use [GitHub](https://github.com/) for versioning.

## Authors

* **Su Su Wai** - *Initial work* - [armitage03](https://github.com/armitage03)

## License

This project is licensed under the MIT License - see the [LICENSE.md](LICENSE.md) file for details
