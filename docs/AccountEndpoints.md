### Endpoints: /account

#### Confirm email address

```http
  GET /api/account/confirm-email/{userId}/{token}
```

| Parameter | Type     | Required |
| :-------- | :------- | :------- |
| `userId` | `string` | Yes |
| `token` | `string` | Yes |

#### Change email address

```http
  POST /api/account/change-email
```

| Parameter | Type     | Required |
| :-------- | :------- | :------- |
| `email` | `string` | Yes |
| `newEmail` | `string` | Yes |

#### Confirm email address change

```http
  GET /api/account/confirm-email-change/{userId}/{email}/{token}
```

| Parameter | Type     | Required |
| :-------- | :------- | :------- |
| `userId` | `string` | Yes |
| `email` | `string` | Yes |
| `token` | `string` | Yes |
