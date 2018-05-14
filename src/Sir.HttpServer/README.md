# Sir

HTTP data endpoint with pluggable read and write pipelines. 

Useful for when you want to

- aggregate data from many sources using a single query language
- dispatch data to many targets
- understand many formats

## Programmability

Readers and writers form read and write pipelines that execute in parallel and independently of each other.

Model formatters, model and query parsers are mapped to a specific media type.

- Provide your own writer to dispatch your data to many stores or to a particular store.
- Provide your own reader to aggregate data from many sources or read from a specific database.
- Add support for your favorite format by plugging in your custom model parser and formatter.
- Add support for your favorite query language by plugging in your custom query parser.