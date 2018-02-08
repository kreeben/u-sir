# U-sir

U-sir is a data store and map/reducer that consists of _read_, _write_ and _data_ servers which may be run on a single machine or distributed over several.

U-sir has a HTTP API, a programmable read and write pipeline and can support any type of data format through its use of custom model binders.

Main features:

- Construct queries by _programming your own map/reduce procedures_. 
- Add support for your own data formats by _authoring your own model binders_. 
- Modify the write pipeline by _creating your own analyzers_.

## Full-text search map/reducer

Out-of-the-box U-sir is configured as a full-text search engine. 

- Map/reducer acts as a index scanner and search results aggregator.
- Model binder for ASCII character-separated values (text files with values separated by ASCII char 30 and 31).
- Full-text analyzer registered in the write pipeline that maintains a full-text index and dynamically forms a query vocabulary.

## HTTP API

Insert, update, delete and query your data using the HTTP read and write endoints.

## Writing

Insert and update data by issuing HTTP POST commands to a writer endpoint with a data file as the payload. First URL segment after host should be the name of the IWriter implementation you want to execute. Leave out the writer name from the URL to execute all writers.

### Example of write command that executes the built-in text writer

	HTTP POST http://uwrite.company.com/text (text/plain, filename: sales.txt)

The writer will

- use the file name of the posted file as a table name
- append the payload (request stream) to a segment in a log file
- index the location in the log of each row of each file and register their state
- find a model binder mapped to media type 'text/plain' and run it over the request stream to get a model
- run all analyzers (IAnalyzer) over the model

### Content types

Model binders take care of the work needed to add support for a file format. The standard model binder maps to media type `text/plain` and ASCII delimited text files, a type of text files with tabular data. 

#### ASCII delimited text file - special formatting

- On the first row of the file please print column names. 
- On second row print data type names (datetime, integer, decimal, string). 
- Third row should be start of data.
- Separate rows with ASCII character 30 and members of rows with ASCII character 31.
- Encode in UTF-8. 

#### Other content types

To add support for other content types you may implement your own model binder. It should expose the data in the HTTP request stream to the analyzer as a list of vectors, a tuple for the column labels. 

### Analyzer pipeline

The standard analyzer

- maintains a full-text search index of all the data
- lexes, parses and semantically classifies all words, including those that are not string
- links classes to table and column names to form a vocabulary, a query language

The standard writer executes all services of type IAnalyzer. You may implement your own analyzers and run many serially.

## Reading

Read data by issuing HTTP GET commands to the read endpoint. In the url you must reference a name of a map/reducer, i.e. a service that implements IMapReduce. 

U-sir comes with a full-text search map/reducer built in.

### Example of search command using the built-in search map/reducer

	HTTP GET http://uread.company.com/search?q=flights%20from%20copenhagen%20to%20paris
