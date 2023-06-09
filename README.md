# Vishkar.Wow
Project to mine data out of WoW auction house using Kafka. This is more of a proof of concept on C# with Kafka but with the World of Warcraft auction house as the main driver.

## Setup

You will need to get Kafka running to get this project running. Many of the tutorials assume, cloud, or linux or a java-first environment. I am focused on a windows desktop based environment with Visual Studio.

You will need to have Java installed. To determine if java is installed open a command prompt and enter `java -version`. If it installed you will get a prompt similar to below:

```
C:\>java -version

openjdk version "11.0.16.1" 2022-08-12 LTS
OpenJDK Runtime Environment Microsoft-40648 (build 11.0.16.1+1-LTS)
OpenJDK 64-Bit Server VM Microsoft-40648 (build 11.0.16.1+1-LTS, mixed mode)
```

If that doesn't work, run the Visual Studio Installer and try and get it installed through that.

Next you will need to get Kafka working.

Download kafka: https://kafka.apache.org/downloads

You will need 7zip or some other tool to extract tgz and tars.

You will likely need to _"unblock"_ the file and then extract the tgz file to a tar file.

```
kafka_2.13-3.4.0.tgz ->
  kafka_2.13-3.4.0.tar
```

I created a folder `C:\kafka`

Next extract the _tar_ to that folder `C:\kafka\kafka_2.13-3.4.0\`

You need to run something called _zookeeper_ which seems to be some sort of host that can run a java app (i.e. kafka). To make it easier to run, add the kafka path to the environment variables.

```
c:\kafka\kafka_2.13-3.4.0\bin\windows
```

This makes all the batch files (.bat) in the directory easier to launch from the command prompt. Essentially a usings but for the command prompt.

Browse to the kafka version directory and run the following command.

```
c:\kafka\kafka_2.13-3.4.0>zookeeper-server-start config/zookeeper.properties
```

This runs the `zookeeper-server-start.bat` file fron the `...bin\windows` you mapped to earlier, using the configuration located at `c:\kafka\kafka_2.13-3.4.0\config\zookeeper.properties`.

Finally, we can run kafka. Open another command prompt window, browse to the kafka version directory and run the following command.

```
c:\kafka\kafka_2.13-3.4.0>kafka-server-start config/server.properties
```

To stop execution of either service, have that command prompt be the active window and hit `Ctrl-C` and then `y` to confirm to stop the execution of the batch file.

You will need to create a topic, run the following commands (with kafka running). The second command will confirm that the `--create` was successful.

```
kafka-topics --create --topic commodities-raw --bootstrap-server localhost:9092
kafka-topics --create --topic commodities --bootstrap-server localhost:9092 --partitions 10
kafka-topics --list --bootstrap-server localhost:9092
```

TODO store the kafka config

## Blizzard Setup

Get a clientId and Secret.

Use the following config settings to store the WoW secrets

- VishWow_ClientId
- VishWow_ClientSecret
