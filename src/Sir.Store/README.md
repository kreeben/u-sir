# Sir.Store

Document storage engine.

## File format

### .val

	val val val... 

### .key

	key key key... 

### .doc

	key_id val_id...

### .vix

	(val_id) val_offset val_len val_type

### .kix

	(key_id) key_offset key_len

### .dix

	(doc_id) doc_offset doc_len

### .ix

	keyval_hash/pos_offset pos_len

### .pos

	doc_id doc_id doc_id...