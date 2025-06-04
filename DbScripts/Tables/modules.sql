/****** Object:  Table [fmis].[modules]    Script Date: 6/4/2025 2:23:06 PM ******/
SET ANSI_NULLS ON
SET QUOTED_IDENTIFIER ON
CREATE TABLE [fmis].[modules](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[module_name] [nvarchar](255) NOT NULL,
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

ALTER TABLE [fmis].[modules] ADD  DEFAULT (getdate()) FOR [created_at]
ALTER TABLE [fmis].[modules]  WITH CHECK ADD  CONSTRAINT [FK_Modules_GrandParent] FOREIGN KEY([grand_parent_id])
REFERENCES [fmis].[modules] ([id])
ALTER TABLE [fmis].[modules] CHECK CONSTRAINT [FK_Modules_GrandParent]
ALTER TABLE [fmis].[modules]  WITH CHECK ADD  CONSTRAINT [FK_Modules_Parent] FOREIGN KEY([parent_id])
REFERENCES [fmis].[modules] ([id])
ALTER TABLE [fmis].[modules] CHECK CONSTRAINT [FK_Modules_Parent]