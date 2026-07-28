# でかプランチャー

でかプのグループインスタンス一覧を取得し、ワンクリックで起動できるWindowsデスクトップアプリです。

[![CI](https://github.com/njm2360/dekapu-skill-launcher/actions/workflows/ci.yml/badge.svg)](https://github.com/njm2360/dekapu-skill-launcher/actions/workflows/ci.yml)
![Release](https://img.shields.io/github/v/release/njm2360/dekapu-skill-launcher)
![.NET 10](https://img.shields.io/badge/.NET-10.0-blue)
![Platform](https://img.shields.io/badge/platform-Windows-lightgrey)
[![License](https://img.shields.io/badge/license-BSD--2--Clause-blue)](LICENSE)

---

## 動作環境

- Windows 10 / 11 (x64)
- .NET 10.0 Desktop Runtime

---

## インストール

### リリースから使う (推奨)

1. [Releases](https://github.com/njm2360/dekapu-skill-launcher/releases) から最新の `dekapu-skill-launcher.exe` をダウンロード
2. 任意のフォルダに配置して実行
  ※ .NET 10.0 Desktop Runtime が必要です。未インストールの場合、初回起動時にダウンロードページが開きます。

### ソースからビルド

```bash
git clone https://github.com/njm2360/dekapu-skill-launcher.git
cd dekapu-skill-launcher
dotnet publish -c Release -r win-x64
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
