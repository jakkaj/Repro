Repro
=====

I'm working on a strange bug(s) in Xamarin iOS. 

There are two bugs that may be related, I can only repro one in this code... repro for the other one is available somewhere else and is proving extrememly difficult to pin down.

The first bug is I write something to file, then immediately read it and sometimes the result is different (as if the write did not even work). 

You run each test once, no problem - it works fine. THen do it again (not that I don't clean up the file) and instantly you hit the condition where the files differ. The read text is old... 

I've set up the repro as a unit test

Run in iPhoneSimulator. 

Open up ReproTest.cs and run the tests a couple of time.  You'll see the read and write strings start to differ from each other after a while. 

There are variations proving different assertions around async writing and writing streams versus byte arrays etc. 

Second Bug
==========
The second repro is very hard to pin down. 

Clone https://github.com/jakkaj/Xamling-Core

Set the test project (down bottom) as start up. 

Find a test EntityBucketTests -> TestBucketTypes

Run it a few times. After a could of times, there will be an exception firing in JsonNETEntitySerialiser. It wont error the first couple of times. 

Have a look at entity... it's been corrupted. If you follow it through, it's not written like that... you can correlate writes and it's fine going to the file system. It's coming back corrupted. 

The end of the json always looks like this: ... teStamp":"2014-10-29T00:04:24.619283Z"}teStamp":"2014-10-29T00:04:24.618703Z"}teStamp":"2014-10-29T00:04:24.173004Z"}

That of course shoudl be valid json, saying DateStamp etc. 

It's like it's added on the end of the file again and again. 

I am completely stumped how to get a better repro sorry. 

Both bugs seem similar, but one is corruption, the other is just not reading or something!