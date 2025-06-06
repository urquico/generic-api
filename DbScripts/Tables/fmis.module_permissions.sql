/****** Object:  Table [fmis].[module_permissions]    Script Date: 6/6/2025 2:13:19 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

SET ANSI_NULLS ON
SET QUOTED_IDENTIFIER ON
CREATE TABLE [fmis].[module_permissions](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[permission_name] [nvarchar](255) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
	[permission_status] [int] NULL,
	[module_id] [int] NULL,
	[created_at] [datetime2](7) NULL,
	[created_by] [int] NULL,
	[updated_at] [datetime2](7) NULL,
	[updated_by] [int] NULL,
	[deleted_at] [datetime2](7) NULL,
	[deleted_by] [int] NULL,
 CONSTRAINT [PK__module_p__3213E83F39BC6C4C] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]

GO

ALTER TABLE [fmis].[module_permissions] ADD  DEFAULT (getdate()) FOR [created_at]
GO

ALTER TABLE [fmis].[module_permissions]  WITH CHECK ADD  CONSTRAINT [FK_Permissions_Modules] FOREIGN KEY([module_id])
REFERENCES [fmis].[modules] ([id])
GO

ALTER TABLE [fmis].[module_permissions] CHECK CONSTRAINT [FK_Permissions_Modules]
GO

