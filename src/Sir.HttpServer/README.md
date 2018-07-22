# Sir.HttpServer

HTTP API and data endpoint framework for when you want to design read and write data pipelines.

- aggregate data from many sources
- dispatch data to many targets
- parse many formats
- 

## Programmability

Model formatters, model and query parsers are mapped to a specific media type.

- Implement your own writer to dispatch your data to many stores or to a particular store.
- Implement your own reader to aggregate data from many sources or read from a specific database.
- Add support for your favorite format by plugging in your custom model parser and binder.
- Add support for your favorite query language by plugging in your custom query parser.

## Standard format and default data store

Included in the box is

- a JSON model formatter and parser
- a standard query parser
- an auto-indexing document database with full-text search