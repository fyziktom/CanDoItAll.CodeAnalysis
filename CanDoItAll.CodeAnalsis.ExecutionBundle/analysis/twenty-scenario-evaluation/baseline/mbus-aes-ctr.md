# mbus-aes-ctr

## Simulated Prompt

A telegram decrypted with AES CTR is wrong. Show me AesCtrCrypto and how the provider chooses it.

## Simulated Agent Approach

Search the AES CTR crypto type and ask for crypto context related to provider selection.

## Query

- Repository: `MBusParser`
- Category: `Specific`
- Query text: `AesCtrCrypto`
- Focus tags: `Crypto`
- Relation hints: `MbusCryptoProvider`, `IMbusCrypto`
- Depth: 2
- Intent: `Auto`
- Precision: `Auto`

## Score

- Rating: `Good`
- Helpfulness score: 0,873
- Expected terms: 2/3
- Expected files: 1/1
- Useful files: 2
- Non-useful files: 0
- Noise term hits: 0
- Token budget ratio: 0,321

## Output Metrics

- Search results: 8
- Seed type: MBus.Extensions.Decryption.AesCtrCrypto
- Seed member: MBus.Extensions.Decryption.AesCtrCrypto.BouncyCastleCrypto(bool, byte[], byte[], byte[])
- Files: 2
- Blocks: 4
- Selected lines: 27
- Estimated tokens: 578
- Usage callers: None
- Usage clusters: None

## Symbol Search Top Results

- `MBus.Extensions.Decryption.AesCtrCrypto.AesCtrCrypto(byte[], byte[])` (Member)
- `MBus.Extensions.Decryption.AesCtrCrypto.BouncyCastleCrypto(bool, byte[], byte[], byte[])` (Member)
- `MBus.Extensions.Decryption.AesCtrCrypto.Decrypt(byte[])` (Member)
- `MBus.Extensions.Decryption.AesCtrCrypto.Encrypt(byte[])` (Member)
- `MBus.Extensions.Decryption.AesCtrCrypto._iv` (Member)
- `MBus.Extensions.Decryption.AesCtrCrypto._key` (Member)
- `MBus.Extensions.Decryption.AesCtrCrypto.mode` (Member)
- `MBus.Extensions.Decryption.AesCtrCrypto` (Type)

## Selected Files

- `MbusParser/Decryption/AesCtrCrypto.cs`: 21/45 lines, 3 blocks
- `MbusParser/Decryption/MbusCryptoProvider.cs`: 6/41 lines, 1 blocks
