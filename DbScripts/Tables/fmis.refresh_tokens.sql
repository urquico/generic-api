/****** Object:  Table [fmis].[refresh_tokens]    Script Date: 6/5/2025 5:06:02 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

SET ANSI_NULLS ON
SET QUOTED_IDENTIFIER ON
CREATE TABLE [fmis].[refresh_tokens](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[user_id] [int] NOT NULL,
	[token] [nvarchar](500) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
	[user_agent] [nvarchar](255) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[ip_address] [nvarchar](50) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[is_revoked] [bit] NULL,
	[expires_at] [datetime2](7) NOT NULL,
	[created_at] [datetime2](7) NULL,
	[created_by] [nvarchar](100) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[revoked_at] [datetime2](7) NULL,
	[revoked_by] [nvarchar](100) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
 CONSTRAINT [PK__refresh___3213E83FB6BA19AF] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]

GO

ALTER TABLE [fmis].[refresh_tokens] ADD  DEFAULT (CONVERT([bit],(0))) FOR [is_revoked]
GO

ALTER TABLE [fmis].[refresh_tokens] ADD  DEFAULT (getutcdate()) FOR [created_at]
GO

ALTER TABLE [fmis].[refresh_tokens]  WITH CHECK ADD  CONSTRAINT [FK_RefreshTokens_Users] FOREIGN KEY([user_id])
REFERENCES [fmis].[users] ([id])
ON DELETE CASCADE
GO

ALTER TABLE [fmis].[refresh_tokens] CHECK CONSTRAINT [FK_RefreshTokens_Users]
GO

