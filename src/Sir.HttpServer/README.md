# Sir

REST data endpoint with pluggable read and write pipelines. 

Useful for when you want to

- aggregate data from many sources
- dispatch data to many targets
- understand many formats

## Programmability

Readers, writers, model formatters, model and query parsers are mapped either to a specific "Content-Type" or 
"Accept" request header or to all of them.

- Program your own writer to dispatch your data to many stores or a particular store.

- Program your own reader to aggregate data from many sources or read from a specific database.

- Add support for your favorite format by plugging in your custom model parser and formatter.

## Roadmap

### IQueryFormatter

Pair up the IQueryParser with a IQueryFormatter to allow conversion to and from any query language so that you can 
accept queries in one language and dispatch them in other languages.

Useful for when you want one query language to rule them all.