# ComfyUI请求方式
基本流程为：
1. POST请求发送工作流
2. 轮询轮询状态
3. 下载图片

## POST请求
请求地址为 http://127.0.0.1:6006/prompt
需要以下参数：
Content-Type: application/json

请求体为以下格式的JSON：
```JSON
{
  "prompt": {},          // 必选：完整的工作流定义（节点、连接、参数）
  "client_id": "",       // 可选：用于 WebSocket 路由，建议填 UUID（本地可省略）
  "extra_data": {},      // 可选：扩展数据（云端鉴权、PNG 元信息等）
  "number": 0            // 可选：队列优先级（0 最高，本地有效，云端忽略）
}
```
prompt填写API格式的工作流（ComfyUI导出为API功能）
返回值示例：
```json
{
    "prompt_id": "d23009c2-15e8-4771-974d-13ff7c16d1a3",
    "number": 3,
    "node_errors": {}
}
```
通过prompt_id可以轮询状态

## 轮询轮询状态
请求地址为 http://127.0.0.1:6006/history/{prompt_id}
通过GET请求获取当前工作流的状态
返回值示例：
```json
// 成功
{
    "d23009c2-15e8-4771-974d-13ff7c16d1a3": {
        "prompt": [
            3,
            "d23009c2-15e8-4771-974d-13ff7c16d1a3",
            {
                "3": {
                    "inputs": {
                        "seed": 122642497149471,
                        "steps": 30,
                        "cfg": 8.0,
                        "sampler_name": "euler",
                        "scheduler": "karras",
                        "denoise": 0.8,
                        "model": [
                            "10",
                            0
                        ],
                        "positive": [
                            "32",
                            0
                        ],
                        "negative": [
                            "32",
                            1
                        ],
                        "latent_image": [
                            "54",
                            0
                        ]
                    },
                    "class_type": "KSampler",
                    "_meta": {
                        "title": "K采样器"
                    }
                },
                "4": {
                    "inputs": {
                        "ckpt_name": "IL/miaomiaoPixel_vPred11_IL.safetensors"
                    },
                    "class_type": "CheckpointLoaderSimple",
                    "_meta": {
                        "title": "Checkpoint加载器（简易）"
                    }
                },
                "6": {
                    "inputs": {
                        "text": "(rpgchara), 1girl, white and pink hair, green eyes, black office clothes, cleavage, (white background:1.2), simple background, multiple views, chibi, \n\nmultiple views, reference sheet, small sprite, rpgmaker sprite,\nsharp pixels, sharp edges, masterpiece, best quality,\nrow_1: front\n(row_2: left:1.1)\nrow_3: right\nrow_4: back",
                        "clip": [
                            "10",
                            1
                        ]
                    },
                    "class_type": "CLIPTextEncode",
                    "_meta": {
                        "title": "CLIP文本编码"
                    }
                },
                "7": {
                    "inputs": {
                        "text": "background, cells, grid, straigt lines, lines, frame, boder, tools, item, weapons, staff, cliping, superposition, cramped, tight, bad, bad feet, text, error, fewer, extra, missing, worst quality, jpeg artifacts, low quality, watermark, unfinished, displeasing, oldest, early, chromatic aberration, signature, artistic error, username, scan, abstract, english text, shiny skin, fumes, fog, clouds, magic, floating light, (blurry), incorrect direction",
                        "clip": [
                            "10",
                            1
                        ]
                    },
                    "class_type": "CLIPTextEncode",
                    "_meta": {
                        "title": "CLIP文本编码"
                    }
                },
                "8": {
                    "inputs": {
                        "samples": [
                            "3",
                            0
                        ],
                        "vae": [
                            "4",
                            2
                        ]
                    },
                    "class_type": "VAEDecode",
                    "_meta": {
                        "title": "VAE解码"
                    }
                },
                "10": {
                    "inputs": {
                        "switch_1": "On",
                        "lora_name_1": "IL/rpgchara_v1_IL.safetensors",
                        "strength_model_1": 1.0000000000000002,
                        "strength_clip_1": 1.5000000000000002,
                        "switch_2": "Off",
                        "lora_name_2": "None",
                        "strength_model_2": 0.0,
                        "strength_clip_2": 0.0,
                        "switch_3": "Off",
                        "lora_name_3": "None",
                        "strength_model_3": 0.0,
                        "strength_clip_3": 0.0,
                        "model": [
                            "4",
                            0
                        ],
                        "clip": [
                            "4",
                            1
                        ]
                    },
                    "class_type": "LoraStackLoader_PoP",
                    "_meta": {
                        "title": "Lora Stack Loader PoP"
                    }
                },
                "14": {
                    "inputs": {
                        "width": 864,
                        "height": 1152,
                        "position": "center",
                        "x_offset": 0,
                        "y_offset": 0,
                        "image": [
                            "8",
                            0
                        ]
                    },
                    "class_type": "ImageCrop+",
                    "_meta": {
                        "title": "🔧 Image Crop"
                    }
                },
                "31": {
                    "inputs": {
                        "control_net_name": "SDXL/controlnet-openpose-sdxl-1.0/openposeSDXL_v10.safetensors"
                    },
                    "class_type": "ControlNetLoader",
                    "_meta": {
                        "title": "加载ControlNet模型"
                    }
                },
                "32": {
                    "inputs": {
                        "strength": 1.2000000000000002,
                        "start_percent": 0.0,
                        "end_percent": 1.0,
                        "positive": [
                            "6",
                            0
                        ],
                        "negative": [
                            "7",
                            0
                        ],
                        "control_net": [
                            "31",
                            0
                        ],
                        "image": [
                            "33",
                            0
                        ],
                        "vae": [
                            "4",
                            2
                        ]
                    },
                    "class_type": "ControlNetApplyAdvanced",
                    "_meta": {
                        "title": "应用ControlNet（旧版高级）"
                    }
                },
                "33": {
                    "inputs": {
                        "image": "Controlnet.png"
                    },
                    "class_type": "LoadImage",
                    "_meta": {
                        "title": "加载图像"
                    }
                },
                "54": {
                    "inputs": {
                        "grow_mask_by": 1,
                        "pixels": [
                            "80",
                            0
                        ],
                        "vae": [
                            "4",
                            2
                        ],
                        "mask": [
                            "55",
                            1
                        ]
                    },
                    "class_type": "VAEEncodeForInpaint",
                    "_meta": {
                        "title": "VAE编码（局部重绘）"
                    }
                },
                "55": {
                    "inputs": {
                        "image": "Grille Sprite chara.png"
                    },
                    "class_type": "LoadImage",
                    "_meta": {
                        "title": "加载图像"
                    }
                },
                "57": {
                    "inputs": {
                        "images": [
                            "14",
                            0
                        ]
                    },
                    "class_type": "PreviewImage",
                    "_meta": {
                        "title": "Cropped"
                    }
                },
                "58": {
                    "inputs": {
                        "num_colors": 32,
                        "pixel_size": 6,
                        "image": [
                            "14",
                            0
                        ]
                    },
                    "class_type": "PixelArtNode",
                    "_meta": {
                        "title": "Pixel Art Node"
                    }
                },
                "59": {
                    "inputs": {
                        "images": [
                            "58",
                            0
                        ]
                    },
                    "class_type": "PreviewImage",
                    "_meta": {
                        "title": "Palette resized"
                    }
                },
                "62": {
                    "inputs": {
                        "width": 144,
                        "height": 192,
                        "upscale_method": "nearest-exact",
                        "keep_proportion": "crop",
                        "pad_color": "0, 0, 0",
                        "crop_position": "center",
                        "divisible_by": 1,
                        "device": "gpu",
                        "image": [
                            "99",
                            0
                        ]
                    },
                    "class_type": "ImageResizeKJv2",
                    "_meta": {
                        "title": "Resize Image v2"
                    }
                },
                "66": {
                    "inputs": {
                        "image": "Inpaint.png"
                    },
                    "class_type": "LoadImage",
                    "_meta": {
                        "title": "加载图像"
                    }
                },
                "68": {
                    "inputs": {
                        "images": [
                            "62",
                            0
                        ]
                    },
                    "class_type": "PreviewImage",
                    "_meta": {
                        "title": "Downscaled"
                    }
                },
                "70": {
                    "inputs": {
                        "output_path": "C:\\Users\\aurel\\Documents\\ComfyUI\\output\\Char Sprite RPG\\[time(%Y-%m-%d)]",
                        "filename_prefix": "Char",
                        "filename_delimiter": "_",
                        "filename_number_padding": 4,
                        "filename_number_start": "false",
                        "extension": "png",
                        "dpi": 300,
                        "quality": 100,
                        "optimize_image": "true",
                        "lossless_webp": "true",
                        "overwrite_mode": "false",
                        "show_history": "false",
                        "show_history_by_prefix": "true",
                        "embed_workflow": "true",
                        "show_previews": "true",
                        "images": [
                            "62",
                            0
                        ]
                    },
                    "class_type": "Image Save",
                    "_meta": {
                        "title": "Image Save"
                    }
                },
                "80": {
                    "inputs": {
                        "num_colors": 32,
                        "pixel_size": 6,
                        "image": [
                            "66",
                            0
                        ]
                    },
                    "class_type": "PixelArtNode",
                    "_meta": {
                        "title": "Pixel Art Node"
                    }
                },
                "99": {
                    "inputs": {
                        "torchscript_jit": "default",
                        "image": [
                            "58",
                            0
                        ]
                    },
                    "class_type": "InspyrenetRembg",
                    "_meta": {
                        "title": "Inspyrenet Rembg"
                    }
                },
                "100": {
                    "inputs": {
                        "images": [
                            "99",
                            0
                        ]
                    },
                    "class_type": "PreviewImage",
                    "_meta": {
                        "title": "Background removed"
                    }
                }
            },
            {
                "client_id": "1234",
                "create_time": 1774965154133
            },
            [
                "57",
                "59",
                "68",
                "70",
                "100"
            ]
        ],
        "outputs": {
            "57": {
                "images": [
                    {
                        "filename": "ComfyUI_temp_juvgn_00001_.png",
                        "subfolder": "",
                        "type": "temp"
                    }
                ]
            },
            "59": {
                "images": [
                    {
                        "filename": "ComfyUI_temp_pzvca_00001_.png",
                        "subfolder": "",
                        "type": "temp"
                    }
                ]
            },
            "100": {
                "images": [
                    {
                        "filename": "ComfyUI_temp_ahjhk_00001_.png",
                        "subfolder": "",
                        "type": "temp"
                    }
                ]
            },
            "68": {
                "images": [
                    {
                        "filename": "ComfyUI_temp_jzsfl_00001_.png",
                        "subfolder": "",
                        "type": "temp"
                    }
                ]
            },
            "70": {
                "images": [
                    {
                        "filename": "Char_0001.png",
                        "subfolder": "C:\\Users\\aurel\\Documents\\ComfyUI\\output\\Char Sprite RPG\\2026-03-31",
                        "type": "output"
                    }
                ],
                "files": [
                    "/root/autodl-tmp/ComfyUI/output/C:\\Users\\aurel\\Documents\\ComfyUI\\output\\Char Sprite RPG\\2026-03-31/Char_0001.png"
                ]
            }
        },
        "status": {
            "status_str": "success",
            "completed": true,
            "messages": [
                [
                    "execution_start",
                    {
                        "prompt_id": "d23009c2-15e8-4771-974d-13ff7c16d1a3",
                        "timestamp": 1774965154135
                    }
                ],
                [
                    "execution_cached",
                    {
                        "nodes": [],
                        "prompt_id": "d23009c2-15e8-4771-974d-13ff7c16d1a3",
                        "timestamp": 1774965154173
                    }
                ],
                [
                    "execution_success",
                    {
                        "prompt_id": "d23009c2-15e8-4771-974d-13ff7c16d1a3",
                        "timestamp": 1774965188210
                    }
                ]
            ]
        },
        "meta": {
            "57": {
                "node_id": "57",
                "display_node": "57",
                "parent_node": null,
                "real_node_id": "57"
            },
            "59": {
                "node_id": "59",
                "display_node": "59",
                "parent_node": null,
                "real_node_id": "59"
            },
            "100": {
                "node_id": "100",
                "display_node": "100",
                "parent_node": null,
                "real_node_id": "100"
            },
            "68": {
                "node_id": "68",
                "display_node": "68",
                "parent_node": null,
                "real_node_id": "68"
            },
            "70": {
                "node_id": "70",
                "display_node": "70",
                "parent_node": null,
                "real_node_id": "70"
            }
        }
    }
}

// 还未完成则返回空JSON
{}

```

## 下载图片
请求地址 为 http://127.0.0.1:6006/view
需要以下参数
- filename
- subfolder
- type (output,input,temp)

返回结果为图片的二进制数据表示




# 总结
流程：
1. POST请求/prompt，设置body中的prompt和client_id
2. 使用GET方法轮询/history/{prompt_id}，获取当前工作流的状态，如果还未完成则返回空json，否则访问结果中的output字段
3. 访问/view，填写filename、subfolder和type三个参数（注意一定要正确填写，否则会提示404错误）
POST请求的json模板：
```json
{
    "prompt": {},
    "client_id": "111"
}
```