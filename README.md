# CodingChallengeWordCount
This challenge is to build your own version of the Unix command line tool wc!


Following options are supported.
```shell
# Byte Count
./WordCount -c test.txt
  342190
```

```shell
# Character Count
./WordCount -m test.txt
  339291
```

```shell
# Line count
./WordCount -l test.txt
  7145
```

```shell
# Word Count
./WordCount -w test.txt
  58164
```

```shell
# No option provided
./WordCount test.txt
   Lines: 7145 Words: 58164 Characters: 339291 Bytes: 342190
```

```shell
# Read from standard input
cat test.txt | ./WordCount -l
   7154
```

This project is inspired from John Crickett's [Coding Challenges.](https://codingchallenges.fyi/challenges/challenge-wc/)