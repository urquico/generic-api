# JWT Authentication & Permission-Based Authorization Guide

This document summarizes the implementation of secure JWT authentication (with HttpOnly cookies) and dynamic permission-based authorization in your .NET 8 Web API project.

---

## 1. JWT Authentication Flow

### Login

- User submits credentials to the `/api/v1/auth/login` endpoint.
- Credentials are validated against the database.
- On success:
  - An **access token** (JWT) is generated, containing user info and all permissions as `permission` claims.
  - A **refresh token** is generated and stored in the database.
  - Both tokens are set as **HttpOnly cookies** (`accessToken`, `refreshToken`).

### Refresh

- User's client calls `/api/v1/auth/refresh` when the access token expires.
- The backend:
  - Reads the `refreshToken` from the HttpOnly cookie.
  - Validates the refresh token (checks expiry, revocation, and user existence).
  - Issues a new access token (JWT) and sets it as an HttpOnly cookie.

### Logout

- User's client calls `/api/v1/auth/logout`.
- The backend:
  - Revokes the refresh token in the database (sets `RevokedAt`).
  - Deletes both `accessToken` and `refreshToken` cookies from the client.

---

## 2. JWT Storage & Security

- **HttpOnly cookies** are used for both access and refresh tokens, protecting them from JavaScript access (mitigating XSS risks).
- Cookies are set with `SameSite=Strict` and `Secure=false` (should be `true` in production).
- JWTs include all user permissions as separate `permission` claims.

---

## 3. Permission-Based Authorization

### Permission Format

- Permissions use a namespaced format: `Module.Action` (e.g., `UserManagement.GetAllUsers`).
- This allows for clear, scalable, and unique permission names across modules.

### Custom Attribute

- `[PermissionAuthorize("PermissionName")]` is used to protect endpoints.
- The attribute implements `IAuthorizationFilter` and checks if the authenticated user has the required `permission` claim in their JWT.
- If the user lacks the permission, a 403 Forbidden is returned.

### Example Usage

```csharp
[PermissionAuthorize("UserManagement.GetAllUsers")]
public IActionResult GetAllUsers(...) { ... }
```

---

## 4. Additional Features

- **RefreshTokenCleanupService**: Background service that deletes expired refresh tokens daily.
- **CORS**: Configured for development; restrict in production.
- **Swagger**: Enabled for API documentation/testing.

---

## 5. Best Practices

- Always use HttpOnly and Secure cookies for tokens in production.
- Use namespaced permission strings for clarity and scalability.
- Store refresh tokens securely and clean up expired tokens regularly.
- Never expose JWTs to JavaScript on the client.

---

## 6. References

- See `AuthController.cs`, `TokenService.cs`, and `PermissionAuthorizeAttribute.cs` for implementation details.
- For permission management, see your `UserManagementController.cs` and other protected controllers.

---

**This guide documents your secure, scalable authentication and authorization setup.**
