# Sir.Store

In-process document database and free-text search engine.

## File format

### _.val

	val val val... 

### _.vix

	(val_id) val_offset val_len val_type

### _.kmap

	(key_id) key_hash

### .key

	key key key... 

### .docs

	key_id val_id...

### .kix

	(key_id) key_offset key_len key_type

### .dix

	(doc_id) doc_offset doc_len

### .pos

	[doc_id doc_id next_page_offset] [doc_id       ]...

### .ix

	[node][node]...

### .vec

	[char][int][char][int]...