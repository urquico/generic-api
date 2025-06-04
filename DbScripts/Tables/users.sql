/****** Object:  Table [fmis].[users]    Script Date: 6/4/2025 4:32:13 PM ******/
SET ANSI_NULLS ON
SET QUOTED_IDENTIFIER ON
CREATE TABLE [fmis].[users](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[first_name] [nvarchar](100) NOT NULL,
	[middle_name] [nvarchar](100) NULL,
	[last_name] [nvarchar](100) NOT NULL,
	[email] [nvarchar](255) NOT NULL,
	[password] [nvarchar](255) NOT NULL,
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

ALTER TABLE [fmis].[users] ADD  DEFAULT (getdate()) FOR [created_at]
ALTER TABLE [fmis].[users]  WITH CHECK ADD  CONSTRAINT [FK_Users_StatusKeyCategory] FOREIGN KEY([status_id])
REFERENCES [mwss].[key_categories] ([id])
ALTER TABLE [fmis].[users] CHECK CONSTRAINT [FK_Users_StatusKeyCategory]