# Sir

A data aggregator and dispatcher with HTTP API and pluggable read/write pipelines can be useful 
when you want to:

- aggregate data from many sources
- dispatch data to many targets
- parse many formats

## Programmability

Model formatters, model and query parsers are mapped to a specific media type.

- Implement your own writer to dispatch your data to many stores or to a particular store.
- Implement your own reader to aggregate data from many sources or read from a specific database.
- Add support for your favorite format by plugging in your custom model parser and formatter.
- Add support for your favorite query language by plugging in your custom query parser.

## Read/write JSON

Included is a JSON model formatter and parser, a standard query parser and an auto-indexing 
64-bit document database with full-text search, implemented as a Sir.IReader and 
Sir.IWriter.