### Endpoints: /policy

#### List all policies

```http
  GET /api/policy
```

> This endpoint does not require any parameters.

#### Create a new policy

```http
  POST /api/policy/create-policy
```

| Parameter | Type     | Required |
| :-------- | :------- | :------- |
| `policyName` | `string` | Yes |
| `policyDescription` | `string` | Yes |
| `policyPermissions` | `array` | Yes |

##### Delete a policy

```http
  DELETE /api/policy/delete-policy
```

| Parameter | Type     | Required |
| :-------- | :------- | :------- |
| `id` | `string` | Yes |
| `policyName` | `string` | Yes |