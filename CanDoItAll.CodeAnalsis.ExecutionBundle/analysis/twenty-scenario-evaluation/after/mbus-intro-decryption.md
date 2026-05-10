# mbus-intro-decryption

## Simulated Prompt

I need to understand encryption/decryption support before fixing a meter that uses AES CTR.

## Simulated Agent Approach

Search for the crypto provider, then ask focused context with crypto tags and AES-related relation hints.

## Query

- Repository: `MBusParser`
- Category: `Introduction`
- Query text: `MbusCryptoProvider`
- Focus tags: `Crypto`
- Relation hints: `AesCtrCrypto`, `AesCbcCrypto`, `DesCbcCrypto`
- Depth: 2
- Intent: `TroublePath`
- Precision: `Outline`

## Score

- Rating: `Mixed`
- Helpfulness score: 0,547
- Expected terms: 1/3
- Expected files: 1/1
- Useful files: 0
- Non-useful files: 0
- Noise term hits: 0
- Token budget ratio: 0,187

## Output Metrics

- Search results: 4
- Seed type: MBus.Extensions.Decryption.MbusCryptoProvider
- Seed member: MBus.Extensions.Decryption.MbusCryptoProvider.DecryptAes128Cbc(byte[], byte[], byte[])
- Files: 0
- Blocks: 0
- Selected lines: 0
- Estimated tokens: 411
- Usage callers: None
- Usage clusters: None

## Symbol Search Top Results

- `MBus.Extensions.Decryption.MbusCryptoProvider.DecryptAes128Cbc(byte[], byte[], byte[])` (Member)
- `MBus.Extensions.Decryption.MbusCryptoProvider.DecryptAes128Ctr(byte[], byte[], byte[])` (Member)
- `MBus.Extensions.Decryption.MbusCryptoProvider.DecryptDes(byte[], byte[], byte[])` (Member)
- `MBus.Extensions.Decryption.MbusCryptoProvider` (Type)

## Selected Files

