/****** Object:  Table [fmis].[users]    Script Date: 6/5/2025 5:06:04 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

SET ANSI_NULLS ON
SET QUOTED_IDENTIFIER ON
CREATE TABLE [fmis].[users](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[first_name] [nvarchar](100) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
	[middle_name] [nvarchar](100) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[last_name] [nvarchar](100) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
	[email] [nvarchar](255) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
	[password] [nvarchar](255) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
	[status_id] [int] NULL,
	[created_at] [datetime2](7) NULL,
	[created_by] [int] NULL,
	[updated_at] [datetime2](7) NULL,
	[updated_by] [int] NULL,
	[deleted_at] [datetime2](7) NULL,
	[deleted_by] [int] NULL,
 CONSTRAINT [PK__users__3213E83F79457AED] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]

GO

ALTER TABLE [fmis].[users] ADD  DEFAULT (getdate()) FOR [created_at]
GO

ALTER TABLE [fmis].[users]  WITH CHECK ADD  CONSTRAINT [FK_Users_StatusKeyCategory] FOREIGN KEY([status_id])
REFERENCES [mwss].[key_categories] ([id])
GO

ALTER TABLE [fmis].[users] CHECK CONSTRAINT [FK_Users_StatusKeyCategory]
GO

