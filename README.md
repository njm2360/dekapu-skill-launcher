# でかプランチャー

でかプのグループインスタンス一覧を取得し、ワンクリックで起動できるWindowsデスクトップアプリです。

![.NET 8](https://img.shields.io/badge/.NET-8.0-blue) ![Platform](https://img.shields.io/badge/platform-Windows-lightgrey) ![License](https://img.shields.io/badge/license-BSD--2--Clause-blue)

---

## インストール

### リリースから使う (推奨)

1. [Releases](https://github.com/njm2360/dekapu-skill-launcher/releases) から最新の `dekapu-skill-launcher.exe` をダウンロード
2. 任意のフォルダに配置して実行
  ※.NET 8.0 Desktop Runtime が必要です。未インストールの場合、初回起動時にダウンロードページが開きます。

### ソースからビルド

```bash
git clone https://github.com/njm2360/dekapu-skill-launcher.git
cd dekapu-skill-launcher
dotnet publish -c Release -r win-x64 --self-contained
```

---

## 使い方

1. アプリを起動するとインスタンス一覧が自動取得されます（手動更新も可能）
2. 起動したいインスタンスを選択し起動ボタンをクリックします
3. VRChatが起動し、自動で選択したインスタンスにJoinします

### OSCクリックロック機能

スキルブッパ用に左手のクリック状態をロックします

| ボタン | 動作                                              |
| ------ | ------------------------------------------------- |
| ロック | 左手をUse状態にする（ロック状態で押すと再ロック） |
| 解放   | 左手のUse状態を解放する                           |
