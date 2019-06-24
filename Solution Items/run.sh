#!/bin/bash
nohup mono santedb-dcg.exe --name=dcg --appname=fiddler --appsecret=fiddler --console --daemon &
disown
