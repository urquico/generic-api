/****** Object:  Table [fmis].[user_special_permissions]    Script Date: 6/4/2025 4:32:13 PM ******/
SET ANSI_NULLS ON
SET QUOTED_IDENTIFIER ON
CREATE TABLE [fmis].[user_special_permissions](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[user_id] [int] NOT NULL,
	[permission_id] [int] NOT NULL,
	[access_status] [bit] NULL,
	[created_at] [datetime2](7) NULL,
	[created_by] [int] NULL,
	[updated_at] [datetime2](7) NULL,
	[updated_by] [int] NULL,
	[deleted_at] [datetime2](7) NULL,
	[deleted_by] [int] NULL,
 CONSTRAINT [PK__user_spe__3213E83FFBD906A5] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]

ALTER TABLE [fmis].[user_special_permissions] ADD  DEFAULT (getdate()) FOR [created_at]
ALTER TABLE [fmis].[user_special_permissions]  WITH CHECK ADD  CONSTRAINT [FK_UserSpecialPermissions_User] FOREIGN KEY([user_id])
REFERENCES [fmis].[users] ([id])
ON DELETE CASCADE
ALTER TABLE [fmis].[user_special_permissions] CHECK CONSTRAINT [FK_UserSpecialPermissions_User]