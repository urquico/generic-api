/****** Object:  Table [fmis].[user_security_questions]    Script Date: 6/6/2025 1:45:00 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

SET ANSI_NULLS ON
SET QUOTED_IDENTIFIER ON
CREATE TABLE [fmis].[user_security_questions](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[user_id] [int] NOT NULL,
	[security_question_id] [int] NOT NULL,
	[security_answer] [nvarchar](255) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
	[created_at] [datetime2](7) NULL,
	[created_by] [int] NULL,
	[updated_at] [datetime2](7) NULL,
	[updated_by] [int] NULL,
	[deleted_at] [datetime2](7) NULL,
	[deleted_by] [int] NULL,
 CONSTRAINT [PK__user_sec__3213E83FDAD4F491] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]

GO

ALTER TABLE [fmis].[user_security_questions] ADD  DEFAULT (getdate()) FOR [created_at]
GO

ALTER TABLE [fmis].[user_security_questions]  WITH CHECK ADD  CONSTRAINT [FK_UserSecurityQuestions_SecurityQuestions] FOREIGN KEY([security_question_id])
REFERENCES [fmis].[security_questions] ([id])
ON DELETE CASCADE
GO

ALTER TABLE [fmis].[user_security_questions] CHECK CONSTRAINT [FK_UserSecurityQuestions_SecurityQuestions]
GO

ALTER TABLE [fmis].[user_security_questions]  WITH CHECK ADD  CONSTRAINT [FK_UserSecurityQuestions_User] FOREIGN KEY([user_id])
REFERENCES [fmis].[users] ([id])
ON DELETE CASCADE
GO

ALTER TABLE [fmis].[user_security_questions] CHECK CONSTRAINT [FK_UserSecurityQuestions_User]
GO

