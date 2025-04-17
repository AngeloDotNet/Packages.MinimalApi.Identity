### Endpoints: /auth

> This endpoints does not require authentication.

#### Login

```http
  POST /api/auth/login
```

| Parameter | Type     | Required |
| :-------- | :------- | :------- |
| `username` | `string` | Yes |
| `password` | `string` | Yes |
| `rememberme` | `boolean` | Yes |

#### Logout

```http
  POST /api/auth/logout
```

#### Register

```http
  POST /api/auth/register
```

| Parameter | Type     | Required |
| :-------- | :------- | :------- |
| `firstname` | `string` | Yes |
| `lastname` | `string` | Yes |
| `username` | `string` | Yes |
| `email` | `string` | Yes |
| `password` | `string` | Yes |

#### Refresh Token

```http
  POST /api/auth/refresh-token
```

| Parameter | Type     | Required |
| :-------- | :------- | :------- |
| `accessToken` | `string` | Yes |
| `refreshToken` | `string` | Yes |