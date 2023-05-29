# padding-oracle-attack

Playing around with padding oracle attacks in .NET.

More information here: https://learn.microsoft.com/en-us/dotnet/standard/security/vulnerabilities-cbc-mode

![image description](Screenshots/Screenshot%202023-05-29%20143244.png)

## Algorithm

The algorithm was written based on this wiki page: https://en.wikipedia.org/wiki/Padding_oracle_attack. It works the same regardless of which
oracle is used. It resides in Decryptor.cs and is documented there.

The algorithm isn't at all optimised and could definitely be tweaked to prioritise more probable byte values based on the ASCII character
range, resulting in fewer guesses being required.

There's also a bug where it doesn't handle the case where we yield what we think is a padding of 1
(e.g. P'2[K] = 1) but it's actually 2 (e.g. P'2 = ______________22) or 3 (e.g. P'2 = _____________333) etc.

## Oracles

Implemented oracles:
- One which executes the crypto directly and makes decisions based on CryptographicExceptions
- One which executes the crypto directly and makes decisions based on measured timings

More oracles could potentially be implemented. E.g.
- One which triggers decryption via a web app request and makes decisions based on the type of response; e.g. 200 vs 500 caused by padding error
vs 500 caused by successful decryption to syntactically valid but semantically invalid plaintext
- One which triggers decryption via a web app request and monitors timings

## Tested cryptos

- One which uses Rijndael.Create() => attack succeeds with both oracles
- One which uses Aes.Create() => attack succeeds with both oracles
