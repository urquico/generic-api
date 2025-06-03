/****** Object:  Table [mwss].[key_categories]    Script Date: 6/3/2025 3:17:47 PM ******/
SET ANSI_NULLS ON
SET QUOTED_IDENTIFIER ON
CREATE TABLE [mwss].[key_categories](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[category_name] [nvarchar](255) NOT NULL,
	[category_id] [int] NULL,
	[category_value] [nvarchar](255) NULL,
	[created_at] [datetime2](7) NULL,
	[created_by] [int] NULL,
	[updated_at] [datetime2](7) NULL,
	[updated_by] [int] NULL,
	[deleted_at] [datetime2](7) NULL,
	[deleted_by] [int] NULL,
 CONSTRAINT [PK__key_cate__3213E83F82551AA5] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]

/****** Object:  Table [fmis].[users]    Script Date: 6/3/2025 3:17:47 PM ******/
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

/****** Object:  Table [fmis].[refresh_tokens]    Script Date: 6/3/2025 3:17:47 PM ******/
SET ANSI_NULLS ON
SET QUOTED_IDENTIFIER ON
CREATE TABLE [fmis].[refresh_tokens](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[user_id] [int] NOT NULL,
	[token] [nvarchar](500) NOT NULL,
	[user_agent] [nvarchar](255) NULL,
	[ip_address] [nvarchar](50) NULL,
	[is_revoked] [bit] NULL,
	[expires_at] [datetime2](7) NOT NULL,
	[created_at] [datetime2](7) NULL,
	[created_by] [nvarchar](100) NULL,
	[revoked_at] [datetime2](7) NULL,
	[revoked_by] [nvarchar](100) NULL,
 CONSTRAINT [PK__refresh___3213E83FB6BA19AF] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]

ALTER TABLE [mwss].[key_categories] ADD  DEFAULT (getdate()) FOR [created_at]
ALTER TABLE [fmis].[users] ADD  DEFAULT (getdate()) FOR [created_at]
ALTER TABLE [fmis].[refresh_tokens] ADD  DEFAULT (CONVERT([bit],(0))) FOR [is_revoked]
ALTER TABLE [fmis].[refresh_tokens] ADD  DEFAULT (getutcdate()) FOR [created_at]
ALTER TABLE [fmis].[users]  WITH CHECK ADD  CONSTRAINT [FK_Users_StatusKeyCategory] FOREIGN KEY([status_id])
REFERENCES [mwss].[key_categories] ([id])
ALTER TABLE [fmis].[users] CHECK CONSTRAINT [FK_Users_StatusKeyCategory]
ALTER TABLE [fmis].[refresh_tokens]  WITH CHECK ADD  CONSTRAINT [FK_RefreshTokens_Users] FOREIGN KEY([user_id])
REFERENCES [fmis].[users] ([id])
ON DELETE CASCADE
ALTER TABLE [fmis].[refresh_tokens] CHECK CONSTRAINT [FK_RefreshTokens_Users]