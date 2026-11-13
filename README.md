# Product API

An ASP.NET Core **Minimal API** for managing products, built on **.NET 10**. It uses feature-folder (vertical slice) organisation, FluentValidation for request validation, Serilog for logging and HTTP Basic authentication.

## Endpoints

Every endpoint lives under `/product`. The GET endpoints are public. Add and update require HTTP Basic auth and return 401 without valid credentials. The credentials are in the `BasicAuthentication` section of `src/Product.Api/appsettings.json`.

| Method | Route                                    | Description                                                           |
| ------ | ---------------------------------------- | --------------------------------------------------------------------- |
| `GET`  | `/product/list?pageSize=10&pageNumber=1` | Returns a paged list of products                                      |
| `GET`  | `/product?id={id}`                       | Returns a single product (404 if not found)                           |
| `POST` | `/product/add`                           | Adds a product; the caller supplies the id (409 if the id, or the same title + brand, already exists) |
| `POST` | `/product/update`                        | Replaces an existing product (204 on success, 404 if not found, 409 if another product has the same title + brand) |

Errors are returned as ProblemDetails. Invalid requests return 400 with the validation errors.

Two products with the same title and brand count as duplicates. The comparison ignores case and surrounding whitespace.

## Logging

`LoggingMiddleware` is the first middleware in the pipeline. It logs every request's method, path and query string, plus the final response status code, including error responses. Logs are written by Serilog to the console as JSON, configured in the `Serilog` section of `appsettings*.json`.

## Data store

Products are held **in memory**. At startup the API loads them from `src/Product.Data/DataSource/test_products.json`. Adds and updates change only the in-memory list and are never written back to the file, so all changes are lost when the app restarts.

## Running locally

Requires the [.NET 10 SDK](https://dotnet.microsoft.com/download).

```bash
dotnet run --project src/Product.Api
```

The API listens on `http://localhost:5043`. In Development, Swagger UI is at `http://localhost:5043/swagger`; use its **Authorize** button to enter the Basic auth credentials. Sample requests are in `src/Product.Api/Product.Api.http`.

## Tests

`tests/Product.Api.Tests` contains integration tests (xUnit v3, Shouldly, `WebApplicationFactory`). They start the API in memory and call the endpoints over HTTP, with and without Basic auth credentials.

```bash
dotnet test
```
