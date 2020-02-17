CREATE TABLE [dbo].[Transaction](
	[Id] [int] PRIMARY KEY IDENTITY(1,1) NOT NULL,
	[Amount] [decimal](18, 2) NOT NULL,
	[CurrencyCode] [nvarchar](max) NOT NULL,
	[TransactionDate] [datetime] NOT NULL,
	[IsCSV] [bit] NOT NULL,
	[Status] [int] NOT NULL,
	[Code] [nvarchar](max) NOT NULL
 );