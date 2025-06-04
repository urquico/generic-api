/****** Object:  Table [fmis].[user_security_questions]    Script Date: 6/4/2025 2:23:14 PM ******/
SET ANSI_NULLS ON
SET QUOTED_IDENTIFIER ON
CREATE TABLE [fmis].[user_security_questions](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[user_id] [int] NOT NULL,
	[security_question_id] [int] NOT NULL,
	[security_answer] [nvarchar](255) NOT NULL,
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

ALTER TABLE [fmis].[user_security_questions] ADD  DEFAULT (getdate()) FOR [created_at]
ALTER TABLE [fmis].[user_security_questions]  WITH CHECK ADD  CONSTRAINT [FK_UserSecurityQuestions_SecurityQuestions] FOREIGN KEY([security_question_id])
REFERENCES [fmis].[security_questions] ([id])
ON DELETE CASCADE
ALTER TABLE [fmis].[user_security_questions] CHECK CONSTRAINT [FK_UserSecurityQuestions_SecurityQuestions]