/****** Object:  Table [fmis].[roles]    Script Date: 6/3/2025 3:17:51 PM ******/
SET ANSI_NULLS ON
SET QUOTED_IDENTIFIER ON
CREATE TABLE [fmis].[roles](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[role_name] [nvarchar](100) NOT NULL,
	[role_status] [bit] NULL,
	[created_at] [datetime2](7) NULL,
	[created_by] [int] NULL,
	[updated_at] [datetime2](7) NULL,
	[updated_by] [int] NULL,
	[deleted_at] [datetime2](7) NULL,
	[deleted_by] [int] NULL,
 CONSTRAINT [PK__roles__3213E83FF091C0D4] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]

/****** Object:  Table [fmis].[modules]    Script Date: 6/3/2025 3:17:51 PM ******/
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

/****** Object:  Table [fmis].[module_permissions]    Script Date: 6/3/2025 3:17:51 PM ******/
SET ANSI_NULLS ON
SET QUOTED_IDENTIFIER ON
CREATE TABLE [fmis].[module_permissions](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[permission_name] [nvarchar](255) NOT NULL,
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

/****** Object:  Table [fmis].[role_module_permissions]    Script Date: 6/3/2025 3:17:51 PM ******/
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

ALTER TABLE [fmis].[roles] ADD  DEFAULT (getdate()) FOR [created_at]
ALTER TABLE [fmis].[modules] ADD  DEFAULT (getdate()) FOR [created_at]
ALTER TABLE [fmis].[module_permissions] ADD  DEFAULT (getdate()) FOR [created_at]
ALTER TABLE [fmis].[role_module_permissions] ADD  DEFAULT (getdate()) FOR [created_at]
ALTER TABLE [fmis].[modules]  WITH CHECK ADD  CONSTRAINT [FK_Modules_GrandParent] FOREIGN KEY([grand_parent_id])
REFERENCES [fmis].[modules] ([id])
ALTER TABLE [fmis].[modules] CHECK CONSTRAINT [FK_Modules_GrandParent]
ALTER TABLE [fmis].[modules]  WITH CHECK ADD  CONSTRAINT [FK_Modules_Parent] FOREIGN KEY([parent_id])
REFERENCES [fmis].[modules] ([id])
ALTER TABLE [fmis].[modules] CHECK CONSTRAINT [FK_Modules_Parent]
ALTER TABLE [fmis].[module_permissions]  WITH CHECK ADD  CONSTRAINT [FK_Permissions_Modules] FOREIGN KEY([module_id])
REFERENCES [fmis].[modules] ([id])
ALTER TABLE [fmis].[module_permissions] CHECK CONSTRAINT [FK_Permissions_Modules]
ALTER TABLE [fmis].[role_module_permissions]  WITH CHECK ADD  CONSTRAINT [FK_RolePermissions_Permissions] FOREIGN KEY([permission_id])
REFERENCES [fmis].[module_permissions] ([id])
ON DELETE CASCADE
ALTER TABLE [fmis].[role_module_permissions] CHECK CONSTRAINT [FK_RolePermissions_Permissions]
ALTER TABLE [fmis].[role_module_permissions]  WITH CHECK ADD  CONSTRAINT [FK_RolePermissions_Roles] FOREIGN KEY([role_id])
REFERENCES [fmis].[roles] ([id])
ON DELETE CASCADE
ALTER TABLE [fmis].[role_module_permissions] CHECK CONSTRAINT [FK_RolePermissions_Roles]