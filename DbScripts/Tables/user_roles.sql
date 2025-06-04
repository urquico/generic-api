/****** Object:  Table [fmis].[user_roles]    Script Date: 6/4/2025 2:23:14 PM ******/
SET ANSI_NULLS ON
SET QUOTED_IDENTIFIER ON
CREATE TABLE [fmis].[user_roles](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[user_id] [int] NOT NULL,
	[role_id] [int] NOT NULL,
	[created_at] [datetime2](7) NULL,
	[created_by] [int] NULL,
	[updated_at] [datetime2](7) NULL,
	[updated_by] [int] NULL,
	[deleted_at] [datetime2](7) NULL,
	[deleted_by] [int] NULL,
 CONSTRAINT [PK__user_rol__3213E83F4106E08B] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]

ALTER TABLE [fmis].[user_roles] ADD  DEFAULT (getdate()) FOR [created_at]
ALTER TABLE [fmis].[user_roles]  WITH CHECK ADD  CONSTRAINT [FK_user_roles_role] FOREIGN KEY([role_id])
REFERENCES [fmis].[roles] ([id])
ON DELETE CASCADE
ALTER TABLE [fmis].[user_roles] CHECK CONSTRAINT [FK_user_roles_role]