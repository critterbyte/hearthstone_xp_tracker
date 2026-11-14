# การติดตั้ง

> แปลโดยใช้เครื่องช่วย หากเนื้อหาแตกต่างจาก[ฉบับภาษาอังกฤษ](../INSTALL.md) ให้ยึดฉบับภาษาอังกฤษเป็นหลัก ยินดีรับการแก้ไข: [เปิด issue](https://github.com/critterbyte/hearthstone_xp_tracker/issues/new/choose)

<a id="requirements"></a>
## สิ่งที่ต้องมี

- Windows
- [Hearthstone Deck Tracker](https://hsreplay.net/downloads/) (HDT) ทดสอบกับ HDT 1.60.1 แล้ว
- Hearthstone

ปลั๊กอินจะอ่านไฟล์ `Power.log` ของ Hearthstone โดย HDT จะเปิดใช้ล็อกนี้ให้เอง คุณจึงไม่ต้องเปลี่ยนการตั้งค่าใดๆ ใน Hearthstone

<a id="install"></a>
## ติดตั้ง

1. ดาวน์โหลดไฟล์ ZIP ของรุ่นล่าสุดจาก[หน้า Releases](https://github.com/critterbyte/hearthstone_xp_tracker/releases) เช่น `HearthstoneXpTracker-0.1.0.zip`
2. ปิด HDT
3. คลิกขวาที่ไฟล์ ZIP เลือก *คุณสมบัติ* (*Properties*) แล้วเลือก *เลิกบล็อก* (*Unblock*) หากมีตัวเลือกนี้ มิฉะนั้น Windows อาจบล็อกปลั๊กอิน
4. เปิดโฟลเดอร์ปลั๊กอินของ HDT ซึ่งหาได้สองวิธี:
   - ใน HDT: *Settings* (ไอคอนเฟืองทางขวาของแถบด้านบน) *→ Tracker → Plugins → Plugins Folder*
   - ใน File Explorer ของ Windows: พิมพ์ `%APPDATA%\HearthstoneDeckTracker\Plugins` ลงในแถบที่อยู่
5. แตกไฟล์ ZIP ลงในโฟลเดอร์ปลั๊กอิน จะได้โฟลเดอร์ `HearthstoneXpTracker` ที่มีไฟล์เหล่านี้:
   - `HearthstoneXpTracker.dll`
   - `XpTracker.Core.dll`
   - `INSTALL.txt` และ `LICENSE.txt`
6. เปิด HDT
7. เปิด *Settings → Tracker → Plugins* แล้วเปิดใช้งาน **Hearthstone XP Tracker**

โอเวอร์เลย์จะปรากฏในเทิร์นแรกของเกมถัดไปของคุณ ดูว่าโอเวอร์เลย์แสดงอะไรบ้างได้ที่ [USAGE.md](USAGE.md)

ไฟล์ DLL ทั้งสองไฟล์ต้องอยู่ในโฟลเดอร์เดียวกัน หากไม่มี `XpTracker.Core.dll` ปลั๊กอินจะไม่เริ่มทำงาน

แต่ละรุ่นจะระบุค่า checksum SHA-256 ของไฟล์ ZIP ไว้ หากต้องการตรวจสอบไฟล์ที่ดาวน์โหลด ให้รัน `Get-FileHash HearthstoneXpTracker-0.1.0.zip` ใน PowerShell แล้วเปรียบเทียบผลลัพธ์

<a id="update"></a>
## อัปเดต

เมื่อมีเวอร์ชันใหม่กว่า ปลั๊กอินจะแจ้งให้คุณทราบ (ดู [USAGE.md](USAGE.md#update-hint)) วิธีอัปเดต:

1. ดาวน์โหลดไฟล์ ZIP ของรุ่นใหม่และเลิกบล็อกไฟล์ (ดูด้านบน)
2. ปิด HDT
3. แตกไฟล์ ZIP ลงในโฟลเดอร์ปลั๊กอินอีกครั้ง และแทนที่ไฟล์เดิม
4. เปิด HDT

ประวัติเกมและการตั้งค่าของคุณจะยังอยู่ เพราะถูกเก็บไว้ในโฟลเดอร์อื่น (ดูด้านล่าง)

<a id="uninstall"></a>
## ถอนการติดตั้ง

1. ใน HDT ให้เปิด *Settings → Tracker → Plugins* แล้วปิดใช้งานปลั๊กอิน
2. ปิด HDT
3. ลบโฟลเดอร์ `%APPDATA%\HearthstoneDeckTracker\Plugins\HearthstoneXpTracker`

ปลั๊กอินเก็บข้อมูลของคุณไว้ใน `%APPDATA%\HearthstoneDeckTracker\XpTracker`:

| ไฟล์ | เนื้อหา |
|---|---|
| `hs_games.csv` | ประวัติเกม |
| `settings.ini` | การตั้งค่าปลั๊กอิน |

หากไม่ต้องการเก็บข้อมูลไว้ ให้ลบโฟลเดอร์นี้ด้วย

<a id="troubleshooting"></a>
## การแก้ไขปัญหา

**ปลั๊กอินไม่อยู่ในรายการ หรือ HDT แสดงข้อผิดพลาดตอนโหลดปลั๊กอิน**

- ตรวจสอบว่าไฟล์ DLL ทั้งสองไฟล์อยู่ใน `%APPDATA%\HearthstoneDeckTracker\Plugins\HearthstoneXpTracker` ไม่ใช่อยู่ลึกลงไปอีกหนึ่งโฟลเดอร์ (เช่น `...\HearthstoneXpTracker\HearthstoneXpTracker`)
- Windows อาจบล็อกไฟล์ที่ดาวน์โหลดจากอินเทอร์เน็ต ให้คลิกขวาที่ไฟล์ DLL แต่ละไฟล์ เลือก *คุณสมบัติ* (*Properties*) และหากมีช่องทำเครื่องหมาย *เลิกบล็อก* (*Unblock*) ให้เลือกช่องนั้นแล้วคลิก *ตกลง* (*OK*) จากนั้นรีสตาร์ท HDT

**โอเวอร์เลย์ไม่ปรากฏ**

- โอเวอร์เลย์จะแสดงเฉพาะในโหมดจัดอันดับ ปกติ ท้าประลอง และลานประลองเท่านั้น โหมดอื่นยังไม่ได้วัด
- ระหว่างช่วงเลือกการ์ดเริ่มเกม โอเวอร์เลย์จะถูกซ่อนไว้ และจะปรากฏในเทิร์นแรก
- ตรวจสอบว่าโอเวอร์เลย์ของ HDT เองทำงานอยู่ เพราะปลั๊กอินวาดลงบนโอเวอร์เลย์ของ HDT

**มีปัญหาอื่น**

โปรด[เปิด issue](https://github.com/critterbyte/hearthstone_xp_tracker/issues/new/choose) และระบุเวอร์ชัน HDT ของคุณ รวมถึงสิ่งที่คุณคาดว่าจะเห็น

<a id="build-from-source"></a>
## สร้างจากซอร์สโค้ด

ดู [CONTRIBUTING.md](../../CONTRIBUTING.md#building)
