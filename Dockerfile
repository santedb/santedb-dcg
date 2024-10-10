# SanteDB Server Dockerfile  
FROM mono:latest
RUN apt-get clean
MAINTAINER "SanteSuite Contributors"
RUN mkdir /santedb
COPY ./bin/Release/ /santedb/
# RUN mv -vf /santedb/Data/*.dataset /santedb/data/
# RUN rm -rf /santedb/Data
WORKDIR /santedb
EXPOSE 9200/tcp
CMD [ "mono", "/santedb/santedb-dcg.exe", "--dllForce", "--console","--daemon" ]