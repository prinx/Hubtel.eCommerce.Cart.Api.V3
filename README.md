# Hubtel.eCommerce.Cart

Shopping Cart API in .NET Core 6

## API Documentation

### Add a new user

```http
POST https://localhost:7150/api/Users
Content-type: application/json

{
    "name": "Caroline",
    "phoneNumber": "233..."
}
```

### Add product

```http
POST https://localhost:7150/api/Products
Content-type: application/json

{
    "name": "Keyboard",
    "unitPrice": 5000,
    "quantityInStock": 20
}
```

### Add item to cart

```http
POST https://localhost:7150/api/CartItems
Content-type: application/json

{
    "productId": 3,
    "userId": 2,
    "quantity": 1
}
```
- Quantity can be negative
- Only cart item quantity will be updated if cart item already exist

### Delete cart item

```http
DELETE https://localhost:7150/api/CartItems/{id}
```

For a full documentation, kindly consult the Swagger link after running the project.

## Swagger link

Use this link Swagger interface after building the project (should open automatically in the browser after a build):

<https://localhost:7150/swagger/index.html>

## Improvments

- Add unit tests
- Kafka producer and consumer

## LICENSE

[MIT](LICENSE)
