# DynamicText
文章に埋込式を設定して、動的に文章を作成します。  
  
## 特徴
UTF-8で記述したテンプレートファイルに値を展開して、動的にドキュメントを作成します。  
  
## 依存関係
* ZoppaDynamicText.dll  
  埋込式の解析を行うライブラリです。  
  
## 使い方
テンプレートファイルとパラメータファイルを作成します、UTF-8エンコードで保存してください。  
テンプレートファイルで文章の構造と埋込式で動的な要素を記述します。  
パラメータファイルで要素の値を記述します。  
コマンドを実行すると埋込式に値を展開したドキュメントを作成します。  
  
#### テンプレートファイル 
```
# #{app_title}
#{app_summary}  
  
## 特徴
  
## 依存関係
  
## 使い方
  
## インストール
  
## 作成情報
* #{author}
* #{affiliation}
* #{mail}
  
## ライセンス
#{license}
```
#### パラメータファイル
```
app_title = "DynamicText";
app_summary = "文章に埋込式を設定して、動的に文章を作成します。";
author = "造田 崇";
affiliation = "ミウラ 第一システムカンパニー";
mail = "takashi.zouta@kkmiura.co.jp";
license = "apache 2.0";
```
#### 実行
```
> .\DynamicText.exe --template .\READ_ME_template.txt --param .\READ_ME_param.txt
```
#### 実行結果
```
# DynamicText
文章に埋込式を設定して、動的に文章を作成します。

## 特徴

## 依存関係

## 使い方

## インストール

## 作成情報
* 造田 崇
* ミウラ 第一システムカンパニー
* takashi.zouta@kkmiura.co.jp

## ライセンス
apache 2.0
```
![画像1](img/img.png)
  
### CSVファイルを使用
パラメータファイルに`CSV`ファイルを与えることで、パラメータに 1レコード単位の配列データを与えることができます。  
テンプレートファイルから参照する場合は`ファイル名`で配列を参照し、`列名`がプロパティ名になります。  
  
#### テンプレートファイル 
```
\{
	"parson": [
		{trim ','}{for r in csv}\{
			"Name": "#{r.Name}",
			"age": #{r.age}
		\},
		{/for}{/trim}
	]
\}
```
#### パラメータファイル
```
Name, age
造田, 39
あいり, 20
まりん, 23
```
#### 実行
```
> .\DynamicText.exe --template .\csv_temp.txt --param .\csv.csv
```
#### 実行結果
```
{
	"parson": [
		{
			"Name": "造田",
			"age": 39
		},
		{
			"Name": "あいり",
			"age": 20
		},
		{
			"Name": "まりん",
			"age": 23
		}
	]
}
```
![画像2](img/img2.png)

### XMLファイルを使用
パラメータファイルに`XML`ファイルを与えることで、パラメータに XMLのオブジェクトを与えることができます。  
  
#### テンプレートファイル 
```
{for proc in 買い物リスト.内容}
id:#{proc.id}
日付:#{proc.日付}
買う物:#{proc.買う物}
金額:#{proc.金額}
----
{/for}
```
#### パラメータファイル
```
<?xml version="1.0" encoding="utf-8" ?>
<買い物リスト>
	<内容 id="1">
		<日付>8月21日</日付>
		<買う物>カレー</買う物>
		<金額>500</金額>
	</内容>
	<内容 id="2">
		<日付>8月22日</日付>
		<買う物>ラーメン</買う物>
		<金額>600</金額>
	</内容>
	<内容 id="3">
		<日付>8月2３日</日付>
		<買う物>寿司</買う物>
		<金額>15000</金額>
	</内容>
</買い物リスト>
```
#### 実行
```
> .\DynamicText.exe --template resources\xml_template.txt --param resources\data.xml
```
#### 実行結果
```
id:1
日付:8月21日
買う物:カレー
金額:500
----
id:2
日付:8月22日
買う物:ラーメン
金額:600
----
id:3
日付:8月2３日
買う物:寿司
金額:15000
----
```
![画像3](img/img3.png)
  
## インストール
ビルドした実行ファイルを適当なパスに配置してください。  
  
## 作成情報
* 造田 崇
* ミウラ 第一システムカンパニー
* takashi.zouta@kkmiura.co.jp
  
## ライセンス
[apache 2.0](https://github.com/zoppa-software/ZoppaDynamicText?tab=Apache-2.0-1-ov-file)
