/****** Object:  Table [fmis].[modules]    Script Date: 6/6/2025 2:13:26 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

SET ANSI_NULLS ON
SET QUOTED_IDENTIFIER ON
CREATE TABLE [fmis].[modules](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[module_name] [nvarchar](255) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
	[grand_parent_id] [int] NULL,
	[parent_id] [int] NULL,
	[created_at] [datetime2](7) NULL,
	[created_by] [int] NULL,
	[updated_at] [datetime2](7) NULL,
	[updated_by] [int] NULL,
	[deleted_at] [datetime2](7) NULL,
	[deleted_by] [int] NULL,
	[module_status] [bit] NULL,
 CONSTRAINT [PK__modules__3213E83F0DE0AD96] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]

GO

ALTER TABLE [fmis].[modules] ADD  DEFAULT (getdate()) FOR [created_at]
GO

ALTER TABLE [fmis].[modules]  WITH CHECK ADD  CONSTRAINT [FK_Modules_GrandParent] FOREIGN KEY([grand_parent_id])
REFERENCES [fmis].[modules] ([id])
GO

ALTER TABLE [fmis].[modules] CHECK CONSTRAINT [FK_Modules_GrandParent]
GO

ALTER TABLE [fmis].[modules]  WITH CHECK ADD  CONSTRAINT [FK_Modules_Parent] FOREIGN KEY([parent_id])
REFERENCES [fmis].[modules] ([id])
GO

ALTER TABLE [fmis].[modules] CHECK CONSTRAINT [FK_Modules_Parent]
GO

