### Endpoints: /claims

#### List all claims

```http
  GET /api/claims
```

> This endpoint does not require any parameters.

##### Create a new claim

```http
  POST /api/claims/create-claim
```

| Parameter | Type     | Required |
| :-------- | :------- | :------- |
| `type` | `string` | Yes |
| `value` | `string` | Yes |

#### Assign claim to user

```http
  POST /api/claims/assign-claim
```

| Parameter | Type     | Required |
| :-------- | :------- | :------- |
| `userId` | `int` | Yes |
| `type` | `string` | Yes |
| `value` | `string` | Yes |

#### Revoke claim from user

```http
  POST /api/claims/revoke-claim
```

| Parameter | Type     | Required |
| :-------- | :------- | :------- |
| `userId` | `int` | Yes |
| `type` | `string` | Yes |
| `value` | `string` | Yes |

#### Delete a claim

```http
  DELETE /api/claims/delete-claim
```

| Parameter | Type     | Required |
| :-------- | :------- | :------- |
| `type` | `string` | Yes |
| `value` | `string` | Yes |