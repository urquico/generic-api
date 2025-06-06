/****** Object:  Table [fmis].[role_module_permissions]    Script Date: 6/6/2025 2:13:29 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

SET ANSI_NULLS ON
SET QUOTED_IDENTIFIER ON
CREATE TABLE [fmis].[role_module_permissions](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[role_id] [int] NOT NULL,
	[permission_id] [int] NOT NULL,
	[created_at] [datetime2](7) NULL,
	[created_by] [int] NULL,
	[updated_at] [datetime2](7) NULL,
	[updated_by] [int] NULL,
	[deleted_at] [datetime2](7) NULL,
	[deleted_by] [int] NULL,
 CONSTRAINT [PK__role_per__3213E83FC80CA33C] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]

GO

ALTER TABLE [fmis].[role_module_permissions] ADD  DEFAULT (getdate()) FOR [created_at]
GO

ALTER TABLE [fmis].[role_module_permissions]  WITH CHECK ADD  CONSTRAINT [FK_RolePermissions_Permissions] FOREIGN KEY([permission_id])
REFERENCES [fmis].[module_permissions] ([id])
ON DELETE CASCADE
GO

ALTER TABLE [fmis].[role_module_permissions] CHECK CONSTRAINT [FK_RolePermissions_Permissions]
GO

ALTER TABLE [fmis].[role_module_permissions]  WITH CHECK ADD  CONSTRAINT [FK_RolePermissions_Roles] FOREIGN KEY([role_id])
REFERENCES [fmis].[roles] ([id])
ON DELETE CASCADE
GO

ALTER TABLE [fmis].[role_module_permissions] CHECK CONSTRAINT [FK_RolePermissions_Roles]
GO

